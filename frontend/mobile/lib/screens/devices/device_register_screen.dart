import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:go_router/go_router.dart';
import '../../core/providers/device_provider.dart';
import '../../core/models/device_model.dart';

class DeviceRegisterScreen extends StatefulWidget {
  const DeviceRegisterScreen({super.key});

  @override
  State<DeviceRegisterScreen> createState() => _DeviceRegisterScreenState();
}

class _DeviceRegisterScreenState extends State<DeviceRegisterScreen> {
  final _formKey = GlobalKey<FormState>();
  final _serialNumberController = TextEditingController();
  final _brandController = TextEditingController();
  final _modelController = TextEditingController();
  final _descriptionController = TextEditingController();
  final _retailerController = TextEditingController();
  final _priceController = TextEditingController();

  String _selectedCategory = 'Smartphone';
  DateTime? _purchaseDate;

  @override
  void dispose() {
    _serialNumberController.dispose();
    _brandController.dispose();
    _modelController.dispose();
    _descriptionController.dispose();
    _retailerController.dispose();
    _priceController.dispose();
    super.dispose();
  }

  Future<void> _selectPurchaseDate() async {
    final date = await showDatePicker(
      context: context,
      initialDate: DateTime.now(),
      firstDate: DateTime(2000),
      lastDate: DateTime.now(),
    );
    if (date != null) {
      setState(() => _purchaseDate = date);
    }
  }

  Future<void> _registerDevice() async {
    if (_formKey.currentState!.validate()) {
      final request = RegisterDeviceRequest(
        serialNumber: _serialNumberController.text,
        category: _selectedCategory,
        brand: _brandController.text.isEmpty ? null : _brandController.text,
        model: _modelController.text.isEmpty ? null : _modelController.text,
        description: _descriptionController.text.isEmpty ? null : _descriptionController.text,
        purchaseDate: _purchaseDate,
        purchasePrice: _priceController.text.isEmpty ? null : double.tryParse(_priceController.text),
        retailer: _retailerController.text.isEmpty ? null : _retailerController.text,
      );

      final provider = context.read<DeviceProvider>();
      final success = await provider.registerDevice(request);

      if (success && mounted) {
        context.go('/devices');
      } else if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(provider.error ?? 'Failed to register device')),
        );
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Register Device'),
      ),
      body: Form(
        key: _formKey,
        child: ListView(
          padding: const EdgeInsets.all(16),
          children: [
            TextFormField(
              controller: _serialNumberController,
              decoration: const InputDecoration(
                labelText: 'Serial Number *',
                border: OutlineInputBorder(),
                helperText: 'Enter the device serial number',
              ),
              validator: (value) {
                if (value == null || value.isEmpty) {
                  return 'Serial number is required';
                }
                return null;
              },
            ),
            const SizedBox(height: 16),
            DropdownButtonFormField<String>(
              value: _selectedCategory,
              decoration: const InputDecoration(
                labelText: 'Category *',
                border: OutlineInputBorder(),
              ),
              items: const [
                DropdownMenuItem(value: 'Smartphone', child: Text('Smartphone')),
                DropdownMenuItem(value: 'Laptop', child: Text('Laptop')),
                DropdownMenuItem(value: 'Tablet', child: Text('Tablet')),
                DropdownMenuItem(value: 'Camera', child: Text('Camera')),
                DropdownMenuItem(value: 'Watch', child: Text('Watch')),
                DropdownMenuItem(value: 'Other', child: Text('Other')),
              ],
              onChanged: (value) {
                if (value != null) {
                  setState(() => _selectedCategory = value);
                }
              },
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _brandController,
              decoration: const InputDecoration(
                labelText: 'Brand',
                border: OutlineInputBorder(),
                hintText: 'e.g., Apple, Samsung',
              ),
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _modelController,
              decoration: const InputDecoration(
                labelText: 'Model',
                border: OutlineInputBorder(),
                hintText: 'e.g., iPhone 15 Pro',
              ),
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _descriptionController,
              decoration: const InputDecoration(
                labelText: 'Description',
                border: OutlineInputBorder(),
                hintText: 'Additional details',
              ),
              maxLines: 3,
            ),
            const SizedBox(height: 16),
            ListTile(
              title: const Text('Purchase Date'),
              subtitle: Text(
                _purchaseDate != null
                    ? '${_purchaseDate!.day}/${_purchaseDate!.month}/${_purchaseDate!.year}'
                    : 'Not selected',
              ),
              trailing: const Icon(Icons.calendar_today),
              onTap: _selectPurchaseDate,
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(4),
                side: BorderSide(color: Colors.grey.shade400),
              ),
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _priceController,
              decoration: const InputDecoration(
                labelText: 'Purchase Price (£)',
                border: OutlineInputBorder(),
                prefixText: '£',
              ),
              keyboardType: TextInputType.number,
            ),
            const SizedBox(height: 16),
            TextFormField(
              controller: _retailerController,
              decoration: const InputDecoration(
                labelText: 'Retailer',
                border: OutlineInputBorder(),
                hintText: 'Where did you buy it?',
              ),
            ),
            const SizedBox(height: 24),
            Consumer<DeviceProvider>(
              builder: (context, provider, _) {
                return ElevatedButton(
                  onPressed: provider.isLoading ? null : _registerDevice,
                  style: ElevatedButton.styleFrom(
                    padding: const EdgeInsets.symmetric(vertical: 16),
                  ),
                  child: provider.isLoading
                      ? const CircularProgressIndicator()
                      : const Text('Register Device'),
                );
              },
            ),
          ],
        ),
      ),
    );
  }
}

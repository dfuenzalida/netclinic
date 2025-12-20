// @jest-environment jsdom
/**
 * Test utilities and helpers for the NetClinic frontend tests
 */
import React from 'react'
import { render, RenderOptions } from '@testing-library/react'

// Common mock functions
export const createMockHashProps = (hash = '#welcome') => ({
  hash,
  setHash: jest.fn()
})

// Mock implementations for common components
export const mockTranslations = {
  'home': 'Home',
  'findOwners': 'Find Owners',
  'findOwner': 'Find Owner', 
  'addOwner': 'Add Owner',
  'lastName': 'Last Name',
  'firstName': 'First Name',
  'address': 'Address',
  'city': 'City',
  'telephone': 'Telephone',
  'vets': 'Veterinarians',
  'petclinicError': 'Something happened...',
  'pets': 'Pets',
  'visits': 'Visits',
  'edit': 'Edit',
  'delete': 'Delete',
  'save': 'Save',
  'cancel': 'Cancel'
}

// Custom render function that includes common providers/wrappers
export const renderWithProviders = (
  ui: React.ReactElement,
  options?: Omit<RenderOptions, 'wrapper'>
) => {
  const Wrapper = ({ children }: { children: React.ReactNode }) => {
    return <div data-testid="test-wrapper">{children}</div>
  }

  return render(ui, { wrapper: Wrapper, ...options })
}

// Helper to create mock API responses
export const createMockOwner = (overrides = {}) => ({
  id: 1,
  firstName: 'John',
  lastName: 'Doe',
  address: '123 Main St',
  city: 'Springfield',
  telephone: '555-0123',
  pets: [],
  ...overrides
})

export const createMockPet = (overrides = {}) => ({
  id: 1,
  name: 'Fluffy',
  type: 'Cat',
  birthDate: '2020-01-01',
  visits: [],
  ...overrides
})

export const createMockVet = (overrides = {}) => ({
  id: 1,
  firstName: 'Dr. Jane',
  lastName: 'Smith',
  specialties: ['Surgery'],
  ...overrides
})

// Helper to wait for async operations
export const waitForAsync = () => new Promise(resolve => setTimeout(resolve, 0))

export * from '@testing-library/react'

// This file is a utility module, not a test file
export const __testUtilities = true
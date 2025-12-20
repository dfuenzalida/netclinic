import React from 'react'
import { render, screen } from '@testing-library/react'
import App from '../page'

// Mock all the components used in the App
jest.mock('../components/NavBar', () => {
  return function NavBar({ hash }: { hash: string }) {
    return <div data-testid="navbar">NavBar - {hash}</div>
  }
})

jest.mock('../components/Welcome', () => {
  return function Welcome() {
    return <div data-testid="welcome">Welcome Component</div>
  }
})

jest.mock('../components/Vets', () => {
  return function Vets() {
    return <div data-testid="vets">Vets Component</div>
  }
})

jest.mock('../components/Owners/OwnerSearchForm', () => {
  return function OwnerSearchForm() {
    return <div data-testid="owner-search-form">Owner Search Form</div>
  }
})

jest.mock('../components/Owners/OwnerSearchResults', () => {
  return function OwnerSearchResults() {
    return <div data-testid="owner-search-results">Owner Search Results</div>
  }
})

jest.mock('../components/Owners/OwnerDetailsView', () => {
  return function OwnerDetailsView() {
    return <div data-testid="owner-details">Owner Details</div>
  }
})

jest.mock('../components/Owners/OwnerCreateEditForm', () => {
  return function OwnerCreateEditForm() {
    return <div data-testid="owner-form">Owner Create/Edit Form</div>
  }
})

jest.mock('../components/Pets/PetCreateEditForm', () => {
  return function PetCreateEditForm() {
    return <div data-testid="pet-form">Pet Create/Edit Form</div>
  }
})

jest.mock('../components/Pets/CreateVisitForm', () => {
  return function CreateVisitForm() {
    return <div data-testid="visit-form">Create Visit Form</div>
  }
})

jest.mock('../components/Oops', () => {
  return function Oops() {
    return <div data-testid="oops">Oops Component</div>
  }
})

// Mock the useHash hook
jest.mock('../components/Hash', () => {
  return function useHash() {
    return ['#welcome', jest.fn()]
  }
})

describe('App Component', () => {
  test('renders welcome component by default', () => {
    render(<App />)
    
    expect(screen.getByTestId('navbar')).toBeInTheDocument()
    expect(screen.getByTestId('welcome')).toBeInTheDocument()
  })

  test('renders the correct layout structure', () => {
    render(<App />)
    
    const containers = document.querySelectorAll('.container-fluid')
    expect(containers).toHaveLength(1)
    
    const innerContainer = document.querySelector('.xd-container')
    expect(innerContainer).toBeInTheDocument()
  })
})

// Test routing logic separately
describe('App Routing', () => {
  beforeEach(() => {
    jest.clearAllMocks()
  })

  test('shows welcome component by default', () => {
    // This is a more realistic test than trying to mock complex routing
    render(<App />)
    expect(screen.getByTestId('welcome')).toBeInTheDocument()
  })
})
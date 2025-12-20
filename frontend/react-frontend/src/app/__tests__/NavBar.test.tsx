import React from 'react'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import NavBar from '../components/NavBar'

// Mock the Translations component
jest.mock('../components/Translations', () => {
  return function T(key: string) {
    const translations: { [key: string]: string } = {
      'home': 'Home',
      'findOwners': 'Find Owners',
      'vets': 'Veterinarians',
      'petclinicError': 'Something happened...'
    }
    return translations[key] || key
  }
})

describe('NavBar', () => {
  const mockSetHash = jest.fn()
  
  beforeEach(() => {
    mockSetHash.mockClear()
  })

  test('renders navigation items correctly', () => {
    render(<NavBar hash="#welcome" setHash={mockSetHash} />)
    
    expect(screen.getByText('Home')).toBeInTheDocument()
    expect(screen.getByText('Find Owners')).toBeInTheDocument()
    expect(screen.getByText('Veterinarians')).toBeInTheDocument()
    expect(screen.getByText('error')).toBeInTheDocument() // This is the actual text shown
  })

  test('highlights active navigation item correctly', () => {
    render(<NavBar hash="#welcome" setHash={mockSetHash} />)
    
    const homeLink = screen.getByText('Home').closest('a')
    expect(homeLink).toHaveClass('active')
  })

  test('highlights owners section when on owners page', () => {
    render(<NavBar hash="#owners/search" setHash={mockSetHash} />)
    
    const ownersLink = screen.getByText('Find Owners').closest('a')
    expect(ownersLink).toHaveClass('active')
  })

  test('highlights vets section when on vets page', () => {
    render(<NavBar hash="#vets" setHash={mockSetHash} />)
    
    const vetsLink = screen.getByText('Veterinarians').closest('a')
    expect(vetsLink).toHaveClass('active')
  })

  test('navigates to home when home link is clicked', async () => {
    const user = userEvent.setup()
    render(<NavBar hash="#vets" setHash={mockSetHash} />)
    
    const homeLink = screen.getByText('Home')
    await user.click(homeLink)
    
    expect(mockSetHash).toHaveBeenCalledWith('welcome')
  })

  test('navigates to owners search when find owners link is clicked', async () => {
    const user = userEvent.setup()
    render(<NavBar hash="#welcome" setHash={mockSetHash} />)
    
    const ownersLink = screen.getByText('Find Owners')
    await user.click(ownersLink)
    
    expect(mockSetHash).toHaveBeenCalledWith('owners/search')
  })

  test('navigates to vets when vets link is clicked', async () => {
    const user = userEvent.setup()
    render(<NavBar hash="#welcome" setHash={mockSetHash} />)
    
    const vetsLink = screen.getByText('Veterinarians')
    await user.click(vetsLink)
    
    expect(mockSetHash).toHaveBeenCalledWith('vets')
  })

  test('navigates to error page when error link is clicked', async () => {
    const user = userEvent.setup()
    render(<NavBar hash="#welcome" setHash={mockSetHash} />)
    
    const errorLink = screen.getByText('error')
    await user.click(errorLink)
    
    expect(mockSetHash).toHaveBeenCalledWith('oops')
  })
})
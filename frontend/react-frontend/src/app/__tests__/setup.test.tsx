import React from 'react'
import { render, screen } from '@testing-library/react'

// Simple test to verify Jest setup is working
describe('Basic Jest Setup', () => {
  test('can render a simple component', () => {
    const TestComponent = () => <div>Hello Test</div>
    render(<TestComponent />)
    
    expect(screen.getByText('Hello Test')).toBeDefined()
  })

  test('can perform basic assertions', () => {
    expect(1 + 1).toBe(2)
    expect('hello').toContain('ell')
  })
})
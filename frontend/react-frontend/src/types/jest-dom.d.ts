/// <reference types="jest" />
/// <reference types="@testing-library/jest-dom" />

declare namespace jest {
  interface Matchers<R> {
    toBeInTheDocument(): R;
    toHaveValue(value: string | number): R;
    toHaveClass(className: string): R;
    toBeDisabled(): R;
    toBeVisible(): R;
    toHaveTextContent(text: string | RegExp): R;
  }
}

export {};
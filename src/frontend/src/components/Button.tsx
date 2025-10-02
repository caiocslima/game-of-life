import React from 'react';
import clsx from 'clsx';

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  className?: string;
  rounded?: 'none' | 'sm' | 'md' | 'lg' | 'full' | 'left' | 'right';
}

export const Button: React.FC<ButtonProps> = ({
  className,
  children,
  rounded = 'md',
  ...props
}) => {
  const baseClassName =
    'px-4 py-2 font-semibold transition-colors disabled:bg-gray-600 disabled:cursor-not-allowed';

  const roundedClass = {
    none: '',
    sm: 'rounded-sm',
    md: 'rounded-md',
    lg: 'rounded-lg',
    full: 'rounded-full',
    left: 'rounded-l-md',
    right: 'rounded-r-md',
  }[rounded];

  return (
    <button {...props} className={clsx(baseClassName, roundedClass, className)}>
      {children}
    </button>
  );
};

import React from 'react';

type NumberInputProps = React.InputHTMLAttributes<HTMLInputElement>;

export const NumberInput: React.FC<NumberInputProps> = ({ ...props }) => {
  const baseClassName =
    'w-16 p-2 bg-gray-700 text-white rounded-r-md text-center focus:outline-none focus:ring-2 focus:ring-indigo-500 disabled:bg-gray-600';

  return <input {...props} type="number" className={baseClassName} />;
};

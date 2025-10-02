export const LoadingSpinner = () => {
  return (
    <div className="absolute inset-0 bg-black bg-opacity-50 flex items-center justify-center z-10">
      <div className="animate-spin rounded-full h-16 w-16 border-b-2 border-green-400"></div>
    </div>
  );
};

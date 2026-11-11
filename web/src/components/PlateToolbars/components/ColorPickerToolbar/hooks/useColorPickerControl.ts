import { useCallback, useState } from 'react';

export function useColorPickerControl() {
	const [isColorPickerOpen, setIsColorPickerOpen] = useState(false);

	const handleCloseColorPicker = useCallback(() => {
		setIsColorPickerOpen(false);
	}, []);

	const handleOpenColorPicker = useCallback(() => {
		setIsColorPickerOpen(true);
	}, []);

	return {
		isColorPickerOpen,
		handleCloseColorPicker,
		handleOpenColorPicker,
	};
}

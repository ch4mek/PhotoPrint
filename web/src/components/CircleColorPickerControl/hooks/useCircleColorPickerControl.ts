import { useCallback, useState } from 'react';

import { UseCircleColorPickerControlProps } from '@/components/CircleColorPickerControl/types';
import { useToast } from '@/context/Toast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useColorPickerMutations } from '@/hooks/useColorPickerMutations';
import { useGetColorPicker } from '@/queries/useGetColorPicker';

export function useCircleColorPickerControl({
	handleApplyColor,
}: UseCircleColorPickerControlProps) {
	const [isColorPickerOpen, setIsColorPickerOpen] = useState(false);
	const toast = useToast();

	const { data: palette = [] } = useGetColorPicker({
		onError: createOnErrorHandler(toast),
	});

	const { createColorMutation, deleteColorMutation } =
		useColorPickerMutations();

	const handleCloseColorPicker = useCallback(() => {
		setIsColorPickerOpen(false);
	}, []);

	const handleOpenColorPicker = useCallback(() => {
		setIsColorPickerOpen(true);
	}, []);

	const handleColorChange = useCallback(
		(newValue: string) => {
			handleApplyColor(newValue);
			handleCloseColorPicker();
		},
		[handleApplyColor]
	);

	return {
		isColorPickerOpen,
		handleCloseColorPicker,
		handleOpenColorPicker,
		handleColorChange,
		createColorMutation,
		deleteColorMutation,
		palette,
	};
}

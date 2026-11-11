import { lightTheme } from '@mms/mms-ui-library/theme';
import { useCallback, useMemo, useState } from 'react';

import { UseColorPickerControlProps } from '@/components/ColorPickerControl/hooks/types';
import { useToast } from '@/context/Toast';
import { getRandomColor } from '@/helpers/getRandomColor';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useColorPickerMutations } from '@/hooks/useColorPickerMutations';
import { useGetColorPicker } from '@/queries/useGetColorPicker';

export function useColorPickerControl({
	randomColorsAmount,
	numberOfColors,
	disabled,
	onChange,
	fieldName,
}: UseColorPickerControlProps) {
	const [isColorPickerOpen, setIsColorPickerOpen] = useState(false);
	const toast = useToast();

	const { data: palette = [] } = useGetColorPicker({
		onError: createOnErrorHandler(toast),
	});

	const { createColorMutation, deleteColorMutation } =
		useColorPickerMutations();

	const randomColors = useMemo(
		() =>
			new Array(randomColorsAmount)
				.fill(lightTheme.palette['background-white'])
				.map(() => getRandomColor()),
		[]
	);

	const viewRandomColors = useMemo(
		() => randomColors.slice(0, numberOfColors),
		[randomColors, numberOfColors]
	);

	const handleCloseColorPicker = useCallback(() => {
		setIsColorPickerOpen(false);
	}, []);

	const handleOpenColorPicker = useCallback(() => {
		setIsColorPickerOpen(true);
	}, []);

	const handleColorChange = useCallback(
		(newValue: string) => {
			if (!disabled) {
				onChange(fieldName, newValue);
				handleCloseColorPicker();
			}
		},
		[disabled, fieldName, onChange]
	);

	return {
		isColorPickerOpen,
		handleCloseColorPicker,
		handleOpenColorPicker,
		handleColorChange,
		palette,
		viewRandomColors,
		createColorMutation,
		deleteColorMutation,
	};
}

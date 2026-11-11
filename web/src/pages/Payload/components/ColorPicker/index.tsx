import { useState, useCallback } from 'react';

import { CircleColorPickerControl } from '@/components/CircleColorPickerControl';
import { useToast } from '@/context/Toast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { UpdateEmployeeColorRequestBody } from '@/pages/Payload/types';
import { useUpdateEmployeePayload } from '@/queries/useUpdateEmpoyeePayload';

import { ColorPickerProps } from './types';

export function ColorPicker({
	initialColor,
	userId,
	disabled,
}: ColorPickerProps) {
	const [color, setColor] = useState(initialColor);
	const toast = useToast();

	const { mutate } = useUpdateEmployeePayload<UpdateEmployeeColorRequestBody>(
		userId,
		{
			onError: createOnErrorHandler(toast),
		}
	);

	const handleApplyColor = useCallback(
		(newColor: string) => {
			setColor(newColor);
			mutate({
				avatarColor: newColor,
			});
		},
		[color]
	);

	return (
		<CircleColorPickerControl
			value={color}
			handleApplyColor={handleApplyColor}
			disabled={disabled}
		/>
	);
}

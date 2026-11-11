import { KeyboardEvent, useCallback } from 'react';

import { BoxRandomColors } from '@/components/BoxRandomColors';
import { ColorPickerControlProps } from '@/components/ColorPickerControl/components/RandomColors/types';
import { RandomColorsWrapper } from '@/components/ColorPickerControl/styles';
import { EVENT_ENTER_KEY } from '@/constants/eventKeys';

export function RandomColors({
	randomColors,
	flexible,
	spaceBetween,
	disabled,
	handleColorChange,
}: ColorPickerControlProps) {
	const handleClick = useCallback(
		(colorItem: string) => () => handleColorChange(colorItem),
		[handleColorChange]
	);

	const handleKeyDown = useCallback(
		(colorItem: string) => (event: KeyboardEvent<HTMLDivElement>) => {
			if (event.key === EVENT_ENTER_KEY) {
				handleColorChange(colorItem);
			}
		},
		[handleColorChange]
	);

	return (
		<RandomColorsWrapper flexible={flexible} spaceBetween={spaceBetween}>
			{randomColors.map((colorItem) => (
				<BoxRandomColors
					key={colorItem}
					color={colorItem}
					onClick={handleClick(colorItem)}
					onKeyDown={handleKeyDown(colorItem)}
					tabIndex={0}
					disabled={disabled}
				/>
			))}
		</RandomColorsWrapper>
	);
}

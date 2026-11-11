import { useTheme } from '@emotion/react';
import { ColorPicker } from '@mms/mms-ui-library/components/Colorpicker';
import { Popover } from '@mms/mms-ui-library/components/Popover';
import { useOutsideClickMany } from '@mms/mms-ui-library/hooks/useOutsideClickMany';
import { spaces } from '@mms/mms-ui-library/theme';
import { useMemo, useRef } from 'react';

import { COLOR_PICKER_OFFSET } from '@/components/CircleColorPickerControl/constants';
import { useCircleColorPickerControl } from '@/components/CircleColorPickerControl/hooks/useCircleColorPickerControl';
import {
	CircleColorPickerButton,
	CircleColorPickerWrapper,
	getCircleColorPickerPopoverStyles,
} from '@/components/CircleColorPickerControl/styles';
import { CircleColorPickerControlProps } from '@/components/CircleColorPickerControl/types';

export function CircleColorPickerControl({
	value,
	handleApplyColor,
	disabled,
}: CircleColorPickerControlProps) {
	const theme = useTheme();

	const colorPickerButton = useRef<HTMLButtonElement>(null);
	const colorPickerPopover = useRef<HTMLDivElement>(null);

	const {
		isColorPickerOpen,
		handleCloseColorPicker,
		handleOpenColorPicker,
		handleColorChange,
		palette,
		createColorMutation,
		deleteColorMutation,
	} = useCircleColorPickerControl({ handleApplyColor });

	const popoverStylesConfig = useMemo(
		() => getCircleColorPickerPopoverStyles(theme),
		[theme]
	);

	useOutsideClickMany<HTMLButtonElement | HTMLDivElement | null>(
		[colorPickerButton, colorPickerPopover],
		handleCloseColorPicker
	);

	return (
		<CircleColorPickerWrapper>
			<CircleColorPickerButton
				color={value}
				onClick={handleOpenColorPicker}
				ref={colorPickerButton}
				disabled={disabled}
			/>
			<Popover
				open={isColorPickerOpen}
				anchorElement={colorPickerButton.current}
				ref={colorPickerPopover}
				distanceToAnchorElement={spaces.xs}
				wrapperStyles={popoverStylesConfig}
				withTransition={false}
				offset={COLOR_PICKER_OFFSET}
				handleCloseDuringScrolling={handleCloseColorPicker}
			>
				<ColorPicker
					defaultValue={value}
					onApply={handleColorChange}
					palette={palette}
					onPaletteCreate={createColorMutation}
					onPaletteDelete={deleteColorMutation}
				/>
			</Popover>
		</CircleColorPickerWrapper>
	);
}

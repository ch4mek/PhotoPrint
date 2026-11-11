import { useTheme } from '@emotion/react';
import { ColorPicker } from '@mms/mms-ui-library/components/Colorpicker';
import { Popover } from '@mms/mms-ui-library/components/Popover';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { useOutsideClickMany } from '@mms/mms-ui-library/hooks/useOutsideClickMany';
import { spaces } from '@mms/mms-ui-library/theme';
import { PropsWithChildren, useMemo, useRef } from 'react';

import { BoxRandomColors } from '@/components/BoxRandomColors';
import { RandomColors } from '@/components/ColorPickerControl/components/RandomColors';
import {
	COLOR_PICKER_MOBILE_OFFSET,
	DEFAULT_NUMBER_OF_COLORS,
} from '@/components/ColorPickerControl/constants';
import { useColorPickerControl } from '@/components/ColorPickerControl/hooks/useColorPickerControl';
import { popUpBreakpoints } from '@/constants/popUp';

import {
	ColorPickerWrapper,
	ColorPickerButton,
	getColorPickerPopoverStyles,
} from './styles';
import type { ColorPickerProps } from './types';

export function ColorPickerControl({
	value,
	randomColorsAmount = DEFAULT_NUMBER_OF_COLORS,
	onChange,
	fieldName,
	disabled,
	flexible = false,
	numberOfColors = DEFAULT_NUMBER_OF_COLORS,
	spaceBetween = false,
}: PropsWithChildren<ColorPickerProps>) {
	const theme = useTheme();

	const isMobileLWidth = useHasScreenWidth(popUpBreakpoints.mobileL);

	const colorPickerButton = useRef<HTMLButtonElement>(null);
	const colorPickerPopover = useRef<HTMLDivElement>(null);

	const {
		isColorPickerOpen,
		handleCloseColorPicker,
		handleOpenColorPicker,
		handleColorChange,
		palette,
		viewRandomColors,
		createColorMutation,
		deleteColorMutation,
	} = useColorPickerControl({
		randomColorsAmount,
		numberOfColors,
		disabled,
		onChange,
		fieldName,
	});

	const popoverStylesConfig = useMemo(
		() => getColorPickerPopoverStyles(theme),
		[theme]
	);

	useOutsideClickMany<HTMLButtonElement | HTMLDivElement | null>(
		[colorPickerButton, colorPickerPopover],
		handleCloseColorPicker
	);

	return (
		<ColorPickerWrapper disabled={!!disabled}>
			<ColorPickerButton
				type="button"
				disabled={!!disabled}
				ref={colorPickerButton}
				onClick={handleOpenColorPicker}
				isActive={isColorPickerOpen}
			>
				<BoxRandomColors color={value} tabIndex={-1} disabled={!!disabled} />
				<Typography variant="m-400">{value}</Typography>
			</ColorPickerButton>
			<RandomColors
				randomColors={viewRandomColors}
				flexible={!!flexible}
				spaceBetween={!!spaceBetween}
				disabled={!!disabled}
				handleColorChange={handleColorChange}
			/>
			<Popover
				open={isColorPickerOpen}
				anchorElement={colorPickerButton.current}
				ref={colorPickerPopover}
				distanceToAnchorElement={spaces.xs}
				wrapperStyles={popoverStylesConfig}
				withTransition={false}
				offset={!isMobileLWidth ? COLOR_PICKER_MOBILE_OFFSET : undefined}
			>
				<ColorPicker
					defaultValue={value}
					onApply={handleColorChange}
					palette={palette}
					onPaletteCreate={createColorMutation}
					onPaletteDelete={deleteColorMutation}
				/>
			</Popover>
		</ColorPickerWrapper>
	);
}

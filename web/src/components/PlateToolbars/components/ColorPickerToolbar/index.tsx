import { useTheme } from '@emotion/react';
import { CompactColorPicker } from '@mms/mms-ui-library/components/CompactColorPicker';
import { Popover } from '@mms/mms-ui-library/components/Popover';
import { useOutsideClickMany } from '@mms/mms-ui-library/hooks/useOutsideClickMany';
import { spaces, ThemeMode } from '@mms/mms-ui-library/theme';
import {
	getPlateEditorRef,
	getPluginType,
	MARK_BG_COLOR,
	MARK_COLOR,
	ToolbarButton,
} from '@udecode/plate';
import {
	MouseEvent,
	useCallback,
	useEffect,
	useMemo,
	useRef,
	useState,
} from 'react';
import { BaseEditor, Editor } from 'slate';
import { ReactEditor } from 'slate-react';

import { ColorPickerControl } from '@/components/PlateToolbars/components/ColorPickerToolbar/components/ColorPickerControl';
import { COLOR_PICKER_OFFSET } from '@/components/PlateToolbars/components/ColorPickerToolbar/constants';
import { getPalette } from '@/components/PlateToolbars/components/ColorPickerToolbar/utils/getPalette';
import { TOOLBAR_BUTTON_CLASSNAME } from '@/components/PlateToolbars/constants';
import {
	generateInvertBackgroundColor,
	generateInvertTextColor,
} from '@/components/PlateToolbars/utils/generateInvertColor';
import {
	EDITOR_ID,
	getDefaultColor,
	platePopoverIds,
} from '@/constants/plateEditor';

import { useColorPickerControl } from './hooks/useColorPickerControl';
import {
	ColorPickerWrapper,
	getColorPickerPopoverContentStyles,
	getColorPickerPopoverStyles,
} from './styles';
import type { ColorPickerToolbarProps } from './types';
import { getUniformStyleFromFragment } from './utils/getUniformStyleFromFragment';

const { colorPickerId } = platePopoverIds;

export function ColorPickerToolbar({
	pluginKey,
	icon,
	tooltip,
}: ColorPickerToolbarProps) {
	const theme = useTheme();
	const isDark = theme.mode === ThemeMode.DARK;
	const editor = getPlateEditorRef(EDITOR_ID);
	const defaultColor = getDefaultColor(theme.mode);

	const [color, setColor] = useState(defaultColor);

	const colorPickerWrapper = useRef<HTMLDivElement>(null);
	const colorPickerPopover = useRef<HTMLDivElement>(null);

	const palette = useMemo(
		() => getPalette(theme, pluginKey),
		[theme, pluginKey]
	);

	const { isColorPickerOpen, handleCloseColorPicker, handleOpenColorPicker } =
		useColorPickerControl();

	const handleSelectColor = useCallback(
		(newColor: string) => {
			if (!editor || !editor.selection) {
				return;
			}

			let convertedColor = newColor;

			ReactEditor.focus(editor as ReactEditor);

			if (pluginKey === MARK_COLOR) {
				convertedColor = generateInvertTextColor(newColor, false);
			}

			if (pluginKey === MARK_BG_COLOR) {
				convertedColor = generateInvertBackgroundColor(newColor, false);
			}

			setColor(convertedColor);
			editor.addMark(getPluginType(editor, pluginKey), convertedColor);

			handleCloseColorPicker();
		},
		[pluginKey, editor, palette]
	);

	const popoverStylesConfig = useMemo(
		() => getColorPickerPopoverStyles(theme),
		[theme]
	);

	const popoverContentStylesConfig = useMemo(
		() => getColorPickerPopoverContentStyles(),
		[]
	);

	const handleUpdateDefaultColor = useCallback(() => {
		if (!editor || !editor.selection) {
			return;
		}
		const baseEditor = editor as BaseEditor;
		const { selection } = baseEditor;

		const fragment = selection ? Editor.fragment(baseEditor, selection) : [];
		const colorCustom = getUniformStyleFromFragment(
			fragment,
			pluginKey,
			defaultColor
		);
		let convertedColor = colorCustom;

		if (pluginKey === MARK_COLOR) {
			convertedColor = generateInvertTextColor(colorCustom, isDark);
		}

		if (pluginKey === MARK_BG_COLOR) {
			convertedColor = generateInvertBackgroundColor(colorCustom, isDark);
		}

		setColor(convertedColor);
	}, [isDark, pluginKey, defaultColor, editor?.selection]);

	const handleColorPickerClick = (e: MouseEvent<HTMLSpanElement>) => {
		e.preventDefault();
		handleOpenColorPicker();
	};

	const handleClearColorClick = useCallback(() => {
		if (!editor || !editor.selection) {
			return;
		}

		editor.removeMark(getPluginType(editor, pluginKey));

		setColor(defaultColor);

		handleCloseColorPicker();
	}, [editor, pluginKey, defaultColor, handleCloseColorPicker]);

	useOutsideClickMany<HTMLButtonElement | HTMLDivElement | null>(
		[colorPickerWrapper, colorPickerPopover],
		handleCloseColorPicker,
		colorPickerId
	);

	useEffect(() => {
		handleUpdateDefaultColor();
	}, [handleUpdateDefaultColor]);

	return (
		<ColorPickerWrapper ref={colorPickerWrapper}>
			<ToolbarButton
				icon={icon}
				tooltip={tooltip}
				onMouseDown={handleColorPickerClick}
				id={pluginKey}
				classNames={{ root: TOOLBAR_BUTTON_CLASSNAME, active: '' }}
			/>

			<Popover
				id={colorPickerId}
				open={isColorPickerOpen}
				anchorElement={colorPickerWrapper.current}
				ref={colorPickerPopover}
				distanceToAnchorElement={spaces.xs}
				wrapperStyles={popoverStylesConfig}
				contentWrapperStyles={popoverContentStylesConfig}
				withTransition={false}
				handleCloseDuringScrolling={handleCloseColorPicker}
				offset={COLOR_PICKER_OFFSET}
				position="bottom-end"
			>
				<CompactColorPicker
					palette={palette}
					value={color}
					onChange={handleSelectColor}
					showPaletteTitle={false}
					controls={<ColorPickerControl handleClick={handleClearColorClick} />}
				/>
			</Popover>
		</ColorPickerWrapper>
	);
}

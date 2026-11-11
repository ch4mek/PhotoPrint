import { ThemeMode } from '../theme';

import { CustomRootBlock } from '@/types/plateEditor';

export const COMMENT_EDITOR_ID = 'comment_editor';
export const EDITOR_ID = 'Text editor';
export const CLONE_EDITOR_ID = 'Clone text editor';
export const DEFAULT_PLATE_EDITOR_PLACEHOLDER = 'Enter your text...';

export const initialEditorData: CustomRootBlock[] = [
	{ type: 'p', children: [{ text: '' }] },
];

export const lightTextColorPalette = [
	'#F17900',
	'#2C3144',
	'#9A9A9A',
	'#DB0000',
	'#B45F05',
	'#F0B501',
	'#96C854',
	'#48A44E',
	'#48A3E9',
	'#2B63B3',
	'#9B5EE1',
	'#5C2D91',
	'#FF67D9',
	'#FF0EC2',
];

export const darkTextColorPalette = [
	'#FEC387',
	'#F3F3F3',
	'#BDBDBD',
	'#FD7878',
	'#CA771F',
	'#FFD146',
	'#96C854',
	'#48A44E',
	'#66B6F3',
	'#3E7AD0',
	'#B687EB',
	'#8D51D1',
	'#FF67D9',
	'#FF0EC2',
];

export const lightBackgroundColorPalette = [
	'#FF8B80',
	'#FFFF56',
	'#B5FF9A',
	'#ACFDFF',
	'#DDBFFF',
	'#FFC0EF',
	'#F1F1F1',
];

export const darkBackgroundColorPalette = [
	'#884D50',
	'#7E6A2E',
	'#3E664D',
	'#4B6C8D',
	'#68568D',
	'#884E85',
	'#404554',
];

export const getDefaultColor = (themeMode: ThemeMode) =>
	themeMode === ThemeMode.LIGHT
		? lightTextColorPalette[1]
		: darkTextColorPalette[1];

export const platePopoverIds = {
	colorPickerId: 'colorpicker-popover',
	toolbarId: 'toolbar-popover',
};

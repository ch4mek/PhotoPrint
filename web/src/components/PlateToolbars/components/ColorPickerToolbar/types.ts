import type { ToolbarButtonProps } from '@udecode/plate';
import { MARK_BG_COLOR, MARK_COLOR } from '@udecode/plate';
import { ReactNode } from 'react';

export interface ColorPickerToolbarProps {
	pluginKey: typeof MARK_COLOR | typeof MARK_BG_COLOR;
	icon: ReactNode;
	tooltip: ToolbarButtonProps['title'];
}

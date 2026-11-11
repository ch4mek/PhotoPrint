import { MouseEventHandler, KeyboardEventHandler } from 'react';

import { DefaultBoxRandomColorsProps } from '@/types/boxRandomColors';

export interface BoxRandomColorProps extends DefaultBoxRandomColorsProps {
	tabIndex: number;
	onClick?: MouseEventHandler<HTMLDivElement>;
	onKeyDown?: KeyboardEventHandler<HTMLDivElement>;
}

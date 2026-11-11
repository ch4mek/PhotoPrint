import { ReactNode } from 'react';

type SpacingType = 'margin' | 'padding';

export interface CollapsibleProps {
	label: string;
	children: ReactNode;
	onClose?: (label: string) => void;
	onOpen?: (label: string) => void;
	hasError?: boolean;
	allowToOpen?: boolean;
	isOpenByDefault?: boolean;
	defaultContentHeight?: number;
	spacingType?: SpacingType;
}

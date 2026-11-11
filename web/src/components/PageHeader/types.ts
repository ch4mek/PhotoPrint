import { ButtonView } from '@mms/mms-ui-library/components/Button/types';
import { ReactNode } from 'react';

interface AddButtonConfig {
	onClick: () => void;
	title?: string;
	disabled?: boolean;
	showTooltip?: boolean;
	tooltipMessage?: string;
	view?: ButtonView;
	isHidden?: boolean;
}

export interface PageHeaderProps {
	title?: string;
	titleAside?: ReactNode;
	backTo?: string;
	addButton?: AddButtonConfig;
	divider?: boolean;
	needWrapDownDesktopXS?: boolean;
	exportComponent?: ReactNode;
}

export interface HeaderContainerProps {
	isWithTitle: boolean;
	needWrapDownDesktopXS: boolean;
}

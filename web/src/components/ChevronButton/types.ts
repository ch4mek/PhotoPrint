import { ActionButtonProps } from '@mms/mms-ui-library/components/ActionButton';

export interface ChevronButtonProps extends Omit<ActionButtonProps, 'onClick'> {
	onClick?: VoidFunction;
	chevronColor?: string;
	activeColor?: string;
}

export interface ChevronButtonWrapperProps extends ActionButtonProps {
	chevronColor?: string;
	activeColor?: string;
}

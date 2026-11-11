import { ActionButtonProps } from '@mms/mms-ui-library/components/ActionButton';
import { JSX } from 'react';

export interface ControlButtonProps extends ActionButtonProps {
	icon: JSX.Element;
	gridArea: string;
}

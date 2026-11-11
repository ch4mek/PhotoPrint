import { ButtonView } from '@mms/mms-ui-library/components/Button/types';

export interface ButtonAddProps {
	onAdd: () => void;
	title?: string;
	disabled?: boolean;
	showTooltip?: boolean;
	tooltipMessage?: string;
	view?: ButtonView;
}

export interface BookButtonInnerProps {
	disabled: boolean;
	onClick: () => void;
	color: 'primary' | 'secondary';
	tooltipTitle: string;
	tooltipText?: string;
}

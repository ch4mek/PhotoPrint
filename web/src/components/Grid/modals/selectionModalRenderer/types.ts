import { RenderModalProps } from '@/components/Grid';

export interface SelectionModalProps extends RenderModalProps<string[]> {
	extraCheckboxes?: {
		label: string;
		onClick: VoidFunction;
		isChecked: boolean;
		isDefaultCheckMark: boolean;
	}[];
}

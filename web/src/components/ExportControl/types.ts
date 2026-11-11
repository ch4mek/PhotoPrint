import { PickerButtonProps } from '@mms/mms-ui-library/components/PickerButton';

export interface ExportControlProps<T>
	extends Pick<PickerButtonProps<T>, 'options' | 'onChange'> {
	modalWidth?: number | 'auto';
	isDisabled?: boolean;
	onPerUserExportToggle?: VoidFunction;
	isPerUserExport?: boolean;
}

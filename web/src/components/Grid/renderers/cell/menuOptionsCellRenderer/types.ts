import { PickerButtonOption } from '@mms/mms-ui-library/components/PickerButton';

export interface MenuOptionsCellRendererOptions<T> {
	options?: PickerButtonOption<T>[] | ((row: T) => PickerButtonOption<T>[]);
	isDisabled?: boolean;
}

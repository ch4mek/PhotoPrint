import { ExportControlProps } from '../../types';

export interface ExportModalProps<T> extends ExportControlProps<T> {
	closePopover: VoidFunction;
}

import { ReactNode } from 'react';

export interface FormWithSaveExitProps {
	isFormDirty: boolean;
	onClose: () => void;
	children: (handleOutsideClose: VoidFunction) => ReactNode;
	title?: string;
	message?: string;
	confirmText?: string;
	cancelText?: string;
	zIndex?: number;
}

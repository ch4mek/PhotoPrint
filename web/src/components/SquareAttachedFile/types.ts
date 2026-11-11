import { ReactNode } from 'react';

import { AppFile } from '@/types/file';

export interface SquareAttachedFileProps {
	file: AppFile;
	handleRemoveClick: VoidFunction;
	fileControl?: ReactNode;
}

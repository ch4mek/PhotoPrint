import { ChangeEvent } from 'react';

export interface ImageUploadProps {
	file: string | null;
	removeFile: VoidFunction;
	handleFileChange: (
		event: ChangeEvent<HTMLInputElement>
	) => undefined | Array<File> | File;
	alt?: string;
	size?: number;
	uploadRadius?: number;
	disabled?: boolean;
}

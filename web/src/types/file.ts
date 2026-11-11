export enum DocumentFileTypes {
	Pdf = 'pdf',
	Txt = 'txt',
	Doc = 'doc',
	Docx = 'docx',
	Jpg = 'jpg',
	Jpeg = 'jpeg',
	Png = 'png',
	Xlsx = 'xlsx',
	Xls = 'xls',
	Pptx = 'pptx',
	Zip = 'zip',
	Md = 'md',
	Rar = 'rar',
}

export interface AppFile {
	fileName: string;
	fileId: string | number;
	imagePreview?: string | null;
	fileType?: string;
	file?: File;
	fileSize?: number;
}

export enum FilePurpose {
	UserAvatar = 'UserAvatar',
	General = 'General',
	Merch = 'Merch',
}

export enum ImageFileSize {
	Original = 'Original',
	Medium = 'Medium',
	Small = 'Small',
	Thumbnail = 'Thumbnail',
}

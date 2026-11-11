import { CONTENT_DISPOSITION_FILE_NAME_REG_EXP } from '@/constants/regexp';

export interface AxiosResponseWithContentDisposition {
	headers: {
		'content-disposition': string;
	};
}

export const getFileNameFromExportRespone = (
	res: AxiosResponseWithContentDisposition
): string | null => {
	const contentDispositionHeaderLine = res.headers[
		'content-disposition'
	] as string;

	if (contentDispositionHeaderLine) {
		const match = contentDispositionHeaderLine.match(
			CONTENT_DISPOSITION_FILE_NAME_REG_EXP
		);

		if (match) {
			return match[1];
		}
	}

	return null;
};

import axios from 'axios';

import { GET_FILE } from '@/constants/queryPaths';
import { triggerFileDownload } from '@/helpers/triggerFileDownload';
import { parseContentType } from '@/queries/utils/parseContentType';

const FALLBACK_CONTENT_TYPE = 'application/octet-stream';

export const useAttachedDocumentDownload = () => {
	const downloadAttachedDocument =
		(fileId: string | number, fileName: string) => async () => {
			try {
				const getFileResponse = await axios.get(`${GET_FILE}/${fileId}`, {
					responseType: 'arraybuffer',
				});

				const parsedContentType = parseContentType(
					getFileResponse.headers['content-type']
				);

				const fileBlob = new Blob([getFileResponse.data], {
					type: parsedContentType || FALLBACK_CONTENT_TYPE,
				});
				triggerFileDownload(fileBlob, fileName);

				return await Promise.resolve();
			} catch (error) {
				return undefined;
			}
		};

	return { downloadAttachedDocument };
};

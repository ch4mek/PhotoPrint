import { useMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';
import { getExtensionFromContentType } from '../utils/getExtensionFromContentType';

import { GET_FILE } from '@/constants/queryPaths';
import { triggerFileDownload } from '@/helpers/triggerFileDownload';

import { DownloadUserAvatarParams } from './types';

export const useDownloadUserAvatar = () =>
	useMutation<
		boolean,
		AxiosError<ServerResponseError>,
		DownloadUserAvatarParams
	>({
		mutationFn: async ({ fileId, fileTitle }) => {
			const response = await axios.get(`${GET_FILE}/${fileId}`, {
				responseType: 'blob',
			});

			const contentType =
				response.headers['content-type'] || response.data.type;

			const extension = getExtensionFromContentType(contentType);
			const fileName = `${fileTitle}.${extension}`;

			triggerFileDownload(response.data, fileName);

			return true;
		},
	});

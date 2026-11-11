import { mimeMap } from '@/constants/contentType';

import { parseContentType } from './parseContentType';

export const getExtensionFromContentType = (contentType?: string): string => {
	const cleanType = parseContentType(contentType)?.toLowerCase();

	return (cleanType && mimeMap[cleanType]) || 'jpg';
};

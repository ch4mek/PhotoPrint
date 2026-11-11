import get from 'lodash/get';

import { TreeNodes } from '@/pages/Wiki/types';

export const extractItem = (path: string, object: TreeNodes) => {
	const pathString = path[0] === '.' ? path.slice(1) : path;

	return get(object, pathString);
};

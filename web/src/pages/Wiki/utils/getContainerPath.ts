import { Option } from '@/pages/Wiki/types';

export const getContainerPath = (node: Option) => {
	const lastIndexOfBracket = node.accessPath.lastIndexOf('[');

	return node.accessPath.slice(0, lastIndexOfBracket);
};

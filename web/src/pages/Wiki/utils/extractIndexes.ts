import { PARSE_INT_RADIX } from '@/pages/Wiki/constants';
import { Option } from '@/pages/Wiki/types';

export const extractIndexes = (draggedItem: Option, dropItem: Option) => {
	let lastIndexOfBracket = draggedItem.accessPath.lastIndexOf('[');
	const dragIndex = parseInt(
		draggedItem.accessPath.slice(lastIndexOfBracket + 1).replace(']', ''),
		PARSE_INT_RADIX
	);

	lastIndexOfBracket = dropItem.accessPath.lastIndexOf('[');
	const dropIndex = parseInt(
		dropItem.accessPath.slice(lastIndexOfBracket + 1).replace(']', ''),
		PARSE_INT_RADIX
	);

	return { dragIndex, dropIndex };
};

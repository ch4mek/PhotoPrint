import { Option } from '@/pages/Wiki/types';

export const isSiblingNode = (draggedItem: Option, dropItem: Option) => {
	let lastIndexOfBracket = draggedItem.accessPath.lastIndexOf('[');
	const dragItemContainerPath = draggedItem.accessPath.slice(
		0,
		lastIndexOfBracket
	);
	lastIndexOfBracket = dropItem.accessPath.lastIndexOf('[');
	const dropItemContainerPath = dropItem.accessPath.slice(
		0,
		lastIndexOfBracket
	);

	return dragItemContainerPath === dropItemContainerPath;
};

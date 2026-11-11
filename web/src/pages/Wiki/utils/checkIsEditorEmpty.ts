import { CustomRootBlock } from '@/types/plateEditor';

const hasNonEmptyText = (node: CustomRootBlock): boolean => {
	if (typeof node.text === 'string') {
		return node.text.trim() !== '';
	}

	if (Array.isArray(node.children)) {
		return (node.children as CustomRootBlock[]).some(hasNonEmptyText);
	}

	return false;
};

export const checkIsEditorEmpty = (data: CustomRootBlock[]) =>
	data.every((block) => {
		if (
			block.type !== 'p' &&
			block.type !== 'h1' &&
			block.type !== 'h2' &&
			block.type !== 'h3' &&
			block.type !== 'h4' &&
			block.type !== 'h5' &&
			block.type !== 'h6'
		) {
			return false;
		}

		return !hasNonEmptyText(block);
	});

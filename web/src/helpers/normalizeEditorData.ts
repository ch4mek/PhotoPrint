import { CustomRootBlock, isTextChild } from '@/types/plateEditor';

export const normalizeEditorData = (
	data?: CustomRootBlock[]
): CustomRootBlock[] =>
	(data || []).map((block) => {
		const updatedBlock = { ...block };

		const children = updatedBlock.children;

		if (
			Array.isArray(children) &&
			children.length === 1 &&
			isTextChild(children[0]) &&
			children[0].text.trim() === ''
		) {
			updatedBlock.children = children.map((child) =>
				isTextChild(child) && child.text.trim() === ''
					? { ...child, text: '' }
					: child
			) as typeof updatedBlock.children;
		}

		return updatedBlock as CustomRootBlock;
	});

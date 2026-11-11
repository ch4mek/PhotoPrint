import { Text } from 'slate';

import { CustomValue, isTextChild } from '@/types/plateEditor';

export const extractPlainText = (value: CustomValue) => {
	const extractTextFromNode = (node: any): string => {
		if (isTextChild(node)) {
			return node.text;
		}

		if (node.children && Array.isArray(node.children)) {
			return node.children.map(extractTextFromNode).join('');
		}

		return '';
	};

	return value.map(extractTextFromNode).join('\n');
};

export const getPlainCharacterCountWithSpaces = (
	editorData: CustomValue
): number => {
	const plainText = extractPlainText(editorData);

	return plainText.length;
};

export const isEditorEmpty = (value: CustomValue) => {
	if (!value || value.length === 0) {
		return true;
	}

	return value.every((block) =>
		block.children.every((child) =>
			Text.isText(child) ? !child.text.trim() : false
		)
	);
};

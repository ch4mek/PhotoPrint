import { MARK_BG_COLOR, MARK_COLOR } from '@udecode/plate';

export const getUniformStyleFromFragment = (
	fragment: any[],
	attribute: typeof MARK_COLOR | typeof MARK_BG_COLOR,
	defaultColor: string
): string => {
	let result: string | undefined;
	let foundText = false;

	const traverse = (nodes: any[]): boolean =>
		!nodes.some((node) => {
			if ('text' in node) {
				foundText = true;

				const value = node[attribute] ?? defaultColor;

				if (result === undefined) {
					result = value;
				} else if (result !== value) {
					return true;
				}
			}

			return Array.isArray(node.children) && !traverse(node.children);
		});

	const isUniform = traverse(fragment);

	if (!isUniform || !foundText) {
		return '';
	}

	return result ?? '';
};

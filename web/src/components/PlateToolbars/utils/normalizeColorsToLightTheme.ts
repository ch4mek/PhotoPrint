import {
	generateInvertBackgroundColor,
	generateInvertTextColor,
} from '@/components/PlateToolbars/utils/generateInvertColor';

export function normalizeColorsToLightTheme(nodes: any[]): any[] {
	return nodes.map((node) => {
		if (node.children) {
			return {
				...node,
				children: normalizeColorsToLightTheme(node.children),
			};
		}

		const newNode = { ...node };

		if (newNode.color) {
			newNode.color = generateInvertTextColor(newNode.color, false);
		}

		if (newNode.backgroundColor) {
			newNode.backgroundColor = generateInvertBackgroundColor(
				newNode.backgroundColor,
				false
			);
		}

		return newNode;
	});
}

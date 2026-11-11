import { Theme } from '@emotion/react';
import { TRenderLeafProps } from '@udecode/plate';

import { RenderWrapper } from '@/components/PlateToolbars/components/renderers/renderLeaf/styles';
import {
	generateInvertBackgroundColor,
	generateInvertTextColor,
} from '@/components/PlateToolbars/utils/generateInvertColor';
import { ThemeMode } from '@/theme';

export function renderLeaf(theme: Theme, defaultColor: string) {
	return function RenderLeafNode(props: TRenderLeafProps) {
		const { leaf, attributes, children } = props;
		const isDark = theme.mode === ThemeMode.DARK;

		const color =
			typeof leaf.color === 'string'
				? generateInvertTextColor(leaf.color, isDark)
				: undefined;

		const backgroundColor =
			typeof leaf.backgroundColor === 'string'
				? generateInvertBackgroundColor(leaf.backgroundColor, isDark)
				: undefined;

		return (
			<RenderWrapper
				{...attributes}
				color={color}
				backgroundColor={backgroundColor}
				defaultColor={defaultColor}
			>
				{children}
			</RenderWrapper>
		);
	};
}

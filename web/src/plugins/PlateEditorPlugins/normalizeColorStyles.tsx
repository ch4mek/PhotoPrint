import {
	createPlateUI,
	ELEMENT_H1,
	ELEMENT_H2,
	ELEMENT_H3,
	ELEMENT_H4,
	ELEMENT_H5,
	ELEMENT_H6,
	ELEMENT_BLOCKQUOTE,
	PlateRenderElementProps,
} from '@udecode/plate';

import { getDefaultColor } from '@/constants/plateEditor';
import { ThemeMode } from '@/theme';

const plateUI = createPlateUI();

const headingElements = [
	ELEMENT_H1,
	ELEMENT_H2,
	ELEMENT_H3,
	ELEMENT_H4,
	ELEMENT_H5,
	ELEMENT_H6,
	ELEMENT_BLOCKQUOTE,
];

export const normalizeColorStyles = (themeMode: ThemeMode) =>
	Object.fromEntries(
		headingElements.map((elementType) => [
			elementType,
			(props: PlateRenderElementProps) => {
				const { attributes, children } = props;
				const defaultColor = getDefaultColor(themeMode);
				const Component = plateUI[elementType];

				return (
					<Component
						{...props}
						attributes={{
							...attributes,
							style: {
								color: defaultColor,
							},
						}}
					>
						{children}
					</Component>
				);
			},
		])
	);

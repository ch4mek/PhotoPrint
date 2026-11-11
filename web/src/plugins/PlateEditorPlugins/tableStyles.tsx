import { useTheme } from '@emotion/react';
import {
	createPlateUI,
	ELEMENT_TD,
	ELEMENT_TH,
	PlateRenderElementProps,
} from '@udecode/plate';

const plateUI = createPlateUI();

export const tableStyles = {
	[ELEMENT_TD]: (props: PlateRenderElementProps) => {
		const { attributes, children } = props;
		const theme = useTheme();
		const Component = plateUI[ELEMENT_TD];

		return (
			<Component
				{...props}
				attributes={{
					...attributes,
					style: {
						border: `1px solid ${theme.palette['text-black']}`,
						backgroundColor: 'transparent',
					},
				}}
			>
				{children}
			</Component>
		);
	},
	[ELEMENT_TH]: (props: PlateRenderElementProps) => {
		const { attributes, children } = props;
		const theme = useTheme();
		const Component = plateUI[ELEMENT_TH];

		return (
			<Component
				{...props}
				attributes={{
					...attributes,
					style: {
						border: `1px solid ${theme.palette['text-black']}`,
						backgroundColor: 'transparent',
					},
				}}
			>
				{children}
			</Component>
		);
	},
};

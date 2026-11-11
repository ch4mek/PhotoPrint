import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { DragIndicatorIcon } from '@mms/mms-ui-library/icons/editor/DragIndicatorIcon';
import {
	ELEMENT_BLOCKQUOTE,
	ELEMENT_CODE_BLOCK,
	ELEMENT_H1,
	ELEMENT_H2,
	ELEMENT_H3,
	ELEMENT_H4,
	ELEMENT_H5,
	ELEMENT_H6,
	ELEMENT_IMAGE,
	ELEMENT_MEDIA_EMBED,
	ELEMENT_OL,
	ELEMENT_PARAGRAPH,
	ELEMENT_TABLE,
	ELEMENT_TODO_LI,
	ELEMENT_UL,
	withDraggables,
} from '@udecode/plate';

import { DragButton } from '../../pages/Wiki/components/PlateEditor/styles';

import { SMALL_ICON_SIZE } from '@/constants/commonStyles';

export const withStyledDraggables = (components: any) =>
	withDraggables(components, [
		{
			keys: [ELEMENT_PARAGRAPH, ELEMENT_UL, ELEMENT_OL],
			level: 0,
		},
		{
			keys: [
				ELEMENT_PARAGRAPH,
				ELEMENT_BLOCKQUOTE,
				ELEMENT_TODO_LI,
				ELEMENT_H1,
				ELEMENT_H2,
				ELEMENT_H3,
				ELEMENT_H4,
				ELEMENT_H5,
				ELEMENT_H6,
				ELEMENT_IMAGE,
				ELEMENT_OL,
				ELEMENT_UL,
				ELEMENT_TABLE,
				ELEMENT_MEDIA_EMBED,
				ELEMENT_CODE_BLOCK,
			],
			onRenderDragHandle: () => (
				<Tooltip text="Drag to move">
					<DragButton type="button">
						<DragIndicatorIcon
							style={{
								width: SMALL_ICON_SIZE,
								height: SMALL_ICON_SIZE,
								color: 'rgba(55, 53, 47, 1)',
							}}
						/>
					</DragButton>
				</Tooltip>
			),
		},
		{
			key: ELEMENT_H1,
			styles: {
				gutterLeft: {
					padding: '2em 0 4px',
					fontSize: '1.875em',
				},
				blockToolbarWrapper: {
					height: '1.3em',
				},
			},
		},
		{
			key: ELEMENT_H2,
			styles: {
				gutterLeft: {
					padding: '1.4em 0 1px',
					fontSize: '1.5em',
				},
				blockToolbarWrapper: {
					height: '1.3em',
				},
			},
		},
		{
			key: ELEMENT_H3,
			styles: {
				gutterLeft: {
					padding: '1em 0 1px',
					fontSize: '1.25em',
				},
				blockToolbarWrapper: {
					height: '1.3em',
				},
			},
		},
		{
			keys: [ELEMENT_H4, ELEMENT_H5, ELEMENT_H6],
			styles: {
				gutterLeft: {
					padding: '0.75em 0 0',
					fontSize: '1.1em',
				},
				blockToolbarWrapper: {
					height: '1.3em',
				},
			},
		},
		{
			keys: [ELEMENT_PARAGRAPH, ELEMENT_UL, ELEMENT_OL],
			styles: {
				gutterLeft: {
					padding: '4px 0 0',
				},
			},
		},
		{
			key: ELEMENT_BLOCKQUOTE,
			styles: {
				gutterLeft: {
					padding: '18px 0 0',
				},
			},
		},
		{
			key: ELEMENT_CODE_BLOCK,
			styles: {
				gutterLeft: {
					padding: '12px 0 0',
				},
			},
		},
	]);

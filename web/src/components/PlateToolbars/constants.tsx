import { BorderAllIcon } from '@mms/mms-ui-library/icons/editor/BorderAllIcon';
import { BorderBottomIcon } from '@mms/mms-ui-library/icons/editor/BorderBottomIcon';
import { BorderClearIcon } from '@mms/mms-ui-library/icons/editor/BorderClearIcon';
import { BorderLeftIcon } from '@mms/mms-ui-library/icons/editor/BorderLeftIcon';
import { BorderRightIcon } from '@mms/mms-ui-library/icons/editor/BorderRightIcon';
import { BorderTopIcon } from '@mms/mms-ui-library/icons/editor/BorderTopIcon';
import { FormatAlignCenterIcon } from '@mms/mms-ui-library/icons/editor/FormatAlignCenterIcon';
import { FormatColorTextIcon } from '@mms/mms-ui-library/icons/editor/FormatColorTextIcon';
import { LinkIcon } from '@mms/mms-ui-library/icons/editor/LinkIcon';
import { Looks1Icon } from '@mms/mms-ui-library/icons/editor/Looks1Icon';
import { Looks2Icon } from '@mms/mms-ui-library/icons/editor/Looks2Icon';
import { Looks3Icon } from '@mms/mms-ui-library/icons/editor/Looks3Icon';
import { Looks4Icon } from '@mms/mms-ui-library/icons/editor/Looks4Icon';
import { Looks5Icon } from '@mms/mms-ui-library/icons/editor/Looks5Icon';
import { Looks6Icon } from '@mms/mms-ui-library/icons/editor/Looks6Icon';
import {
	deleteColumn,
	deleteRow,
	deleteTable,
	ELEMENT_H1,
	ELEMENT_H2,
	ELEMENT_H3,
	ELEMENT_H4,
	ELEMENT_H5,
	ELEMENT_H6,
	insertTable,
	insertTableColumn,
	insertTableRow,
} from '@udecode/plate';

export const GROUP_TOOLBAR_TITLE_HEADINGS = 'Headings';
export const GROUP_TOOLBAR_TITLE_STYLE = 'Style';
export const GROUP_TOOLBAR_TITLE_PARAGRAPH = 'Paragraph';
export const GROUP_TOOLBAR_TITLE_INSERTS = 'Inserts';
export const GROUP_TOOLBAR_TITLE_TABLES = 'Table borders';

export const FORMAT_COLOR_TEXT_TOOLTIP = 'Text color';
export const FONT_DOWNLOAD_TEXT_TOOLTIP = 'Highlight color';

const headingsTextTooltips = {
	headingOne: 'Heading 1',
	headingTwo: 'Heading 2',
	headingThree: 'Heading 3',
	headingFore: 'Heading 4',
	headingFive: 'Heading 5',
	headingSix: 'Heading 6',
};

export const stylesTextTooltips = {
	bold: 'Bold text',
	italic: 'Italic text',
	underline: 'Underlined text',
	strikethrough: 'Strikethrough text',
	superscript: 'Superscript text',
	subscript: 'Subscript text',
};

export const paragraphTextTooltips = {
	unordered: 'Unordered list',
	ordered: 'Ordered list',
	decrease: 'Decrease indent',
	increase: 'Increase indent',
	alignLeft: 'Align left',
	alignCenter: 'Align center',
	alignRight: 'Align right',
	alignJustify: 'Align justify',
};

export const insertsTextTooltips = {
	blockquote: 'Blockquote',
	codeBlock: 'Code block',
	link: 'Link',
	image: 'Image',
	video: 'Video',
};

export const TOOLBAR_BUTTON_CLASSNAME = 'plate-toolbar-button';

export const basicElements = [
	{
		type: ELEMENT_H1,
		isMain: true,
		tooltip: headingsTextTooltips.headingOne,
		icon: <Looks1Icon />,
	},
	{
		type: ELEMENT_H2,
		isMain: false,
		tooltip: headingsTextTooltips.headingTwo,
		icon: <Looks2Icon />,
	},
	{
		type: ELEMENT_H3,
		isMain: false,
		tooltip: headingsTextTooltips.headingThree,
		icon: <Looks3Icon />,
	},
	{
		type: ELEMENT_H4,
		isMain: true,
		tooltip: headingsTextTooltips.headingFore,
		icon: <Looks4Icon />,
	},
	{
		type: ELEMENT_H5,
		isMain: false,
		tooltip: headingsTextTooltips.headingFive,
		icon: <Looks5Icon />,
	},
	{
		type: ELEMENT_H6,
		isMain: false,
		tooltip: headingsTextTooltips.headingSix,
		icon: <Looks6Icon />,
	},
];
export const tableElements = [
	{
		icon: <BorderAllIcon />,
		transform: insertTable,
		isMain: true,
	},
	{
		icon: <BorderClearIcon />,
		transform: deleteTable,
		isMain: false,
	},
	{
		icon: <BorderBottomIcon />,
		transform: insertTableRow,
		isMain: false,
	},
	{
		icon: <BorderTopIcon />,
		transform: deleteRow,
		isMain: false,
	},
	{
		icon: <BorderLeftIcon />,
		transform: insertTableColumn,
		isMain: false,
	},
	{
		icon: <BorderRightIcon />,
		transform: deleteColumn,
		isMain: false,
	},
];

export const previewIcons = {
	[GROUP_TOOLBAR_TITLE_HEADINGS]: <Looks1Icon />,
	[GROUP_TOOLBAR_TITLE_STYLE]: <FormatColorTextIcon />,
	[GROUP_TOOLBAR_TITLE_PARAGRAPH]: <FormatAlignCenterIcon />,
	[GROUP_TOOLBAR_TITLE_INSERTS]: <LinkIcon />,
	[GROUP_TOOLBAR_TITLE_TABLES]: <BorderAllIcon />,
};

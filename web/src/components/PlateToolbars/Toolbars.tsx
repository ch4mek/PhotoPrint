import './tippy.css';

import { CodeAltIcon } from '@mms/mms-ui-library/icons/editor/CodeAltIcon';
import { FormatAlignCenterIcon } from '@mms/mms-ui-library/icons/editor/FormatAlignCenterIcon';
import { FormatAlignJustifyIcon } from '@mms/mms-ui-library/icons/editor/FormatAlignJustifyIcon';
import { FormatAlignLeftIcon } from '@mms/mms-ui-library/icons/editor/FormatAlignLeftIcon';
import { FormatAlignRightIcon } from '@mms/mms-ui-library/icons/editor/FormatAlignRightIcon';
import { FormatBoldIcon } from '@mms/mms-ui-library/icons/editor/FormatBoldIcon';
import { FormatColorTextIcon } from '@mms/mms-ui-library/icons/editor/FormatColorTextIcon';
import { FormatIndentDecreaseIcon } from '@mms/mms-ui-library/icons/editor/FormatIndentDecreaseIcon';
import { FormatIndentIncreaseIcon } from '@mms/mms-ui-library/icons/editor/FormatIndentIncreaseIcon';
import { FormatItalicIcon } from '@mms/mms-ui-library/icons/editor/FormatItalicIcon';
import { FormatListBulletedIcon } from '@mms/mms-ui-library/icons/editor/FormatListBulletedIcon';
import { FormatListNumberedIcon } from '@mms/mms-ui-library/icons/editor/FormatListNumberedIcon';
import { FormatQuoteIcon } from '@mms/mms-ui-library/icons/editor/FormatQuoteIcon';
import { FormatStrikethroughIcon } from '@mms/mms-ui-library/icons/editor/FormatStrikethroughIcon';
import { FormatUnderlinedIcon } from '@mms/mms-ui-library/icons/editor/FormatUnderlinedIcon';
import { HighlightIcon } from '@mms/mms-ui-library/icons/editor/HighlightIcon';
import { SubscriptIcon } from '@mms/mms-ui-library/icons/editor/SubscriptIcon';
import { SuperscriptIcon } from '@mms/mms-ui-library/icons/editor/SuperscriptIcon';
import {
	ELEMENT_BLOCKQUOTE,
	ELEMENT_OL,
	ELEMENT_UL,
	getPluginType,
	indent,
	MARK_BG_COLOR,
	MARK_BOLD,
	MARK_CODE,
	MARK_COLOR,
	MARK_ITALIC,
	MARK_STRIKETHROUGH,
	MARK_SUBSCRIPT,
	MARK_SUPERSCRIPT,
	MARK_UNDERLINE,
	outdent,
	AlignToolbarButton,
	ToolbarButton,
	BlockToolbarButton,
	ListToolbarButton,
	MarkToolbarButton,
	TableToolbarButton,
} from '@udecode/plate';
import { MouseEvent } from 'react';

import { GroupToolbarButtonsWrapper } from '../GroupToolbarButtonsWrapper';

import { CustomVerticalRow } from '@/components/GroupToolbarButtonsWrapper/styles';
import { ColorPickerToolbar } from '@/components/PlateToolbars/components/ColorPickerToolbar';
import { ImageToolbar } from '@/components/PlateToolbars/components/ImageToolbar';
import { LinkToolbar } from '@/components/PlateToolbars/components/LinkToolbar';
import { ToolbarProvider } from '@/context/ToolbarContext';
import { useCustomPlateEditorRef } from '@/types/plateEditor';

import {
	basicElements,
	FONT_DOWNLOAD_TEXT_TOOLTIP,
	FORMAT_COLOR_TEXT_TOOLTIP,
	GROUP_TOOLBAR_TITLE_HEADINGS,
	GROUP_TOOLBAR_TITLE_INSERTS,
	GROUP_TOOLBAR_TITLE_PARAGRAPH,
	GROUP_TOOLBAR_TITLE_STYLE,
	GROUP_TOOLBAR_TITLE_TABLES,
	insertsTextTooltips,
	paragraphTextTooltips,
	previewIcons,
	stylesTextTooltips,
	tableElements,
	TOOLBAR_BUTTON_CLASSNAME,
} from './constants';
import { ToolbarButtonsProps } from './types';

function BasicElementToolbarButtons() {
	const editor = useCustomPlateEditorRef()!;

	return (
		<>
			{basicElements.map(({ type, tooltip, icon }) => (
				<BlockToolbarButton
					key={type}
					type={getPluginType(editor, type)}
					icon={icon}
					tooltip={{ content: tooltip }}
					classNames={{ root: TOOLBAR_BUTTON_CLASSNAME, active: '' }}
				/>
			))}
		</>
	);
}

function IndentToolbarButtons() {
	const editor = useCustomPlateEditorRef()!;

	const handleIndentDecrease = (e: MouseEvent<HTMLSpanElement>) => {
		if (!editor) {
			return;
		}

		outdent(editor);
		e.preventDefault();
	};

	const handleIndentIncrease = (e: MouseEvent<HTMLSpanElement>) => {
		if (!editor) {
			return;
		}

		indent(editor);
		e.preventDefault();
	};

	return (
		<>
			<ToolbarButton
				onMouseDown={handleIndentDecrease}
				icon={<FormatIndentDecreaseIcon />}
				tooltip={{ content: paragraphTextTooltips.decrease }}
				classNames={{ root: TOOLBAR_BUTTON_CLASSNAME, active: '' }}
			/>
			<ToolbarButton
				onMouseDown={handleIndentIncrease}
				icon={<FormatIndentIncreaseIcon />}
				tooltip={{ content: paragraphTextTooltips.increase }}
				classNames={{ root: TOOLBAR_BUTTON_CLASSNAME, active: '' }}
			/>
		</>
	);
}

function ListToolbarButtons() {
	const editor = useCustomPlateEditorRef()!;

	return (
		<>
			<ListToolbarButton
				type={getPluginType(editor, ELEMENT_UL)}
				icon={<FormatListBulletedIcon />}
				tooltip={{ content: paragraphTextTooltips.unordered }}
				classNames={{ root: TOOLBAR_BUTTON_CLASSNAME, active: '' }}
			/>
			<ListToolbarButton
				type={getPluginType(editor, ELEMENT_OL)}
				icon={<FormatListNumberedIcon />}
				tooltip={{ content: paragraphTextTooltips.ordered }}
				classNames={{ root: TOOLBAR_BUTTON_CLASSNAME, active: '' }}
			/>
		</>
	);
}

function AlignToolbarButtons() {
	return (
		<>
			<AlignToolbarButton
				value="left"
				icon={<FormatAlignLeftIcon />}
				tooltip={{ content: paragraphTextTooltips.alignLeft }}
				classNames={{ root: TOOLBAR_BUTTON_CLASSNAME, active: '' }}
			/>
			<AlignToolbarButton
				value="center"
				icon={<FormatAlignCenterIcon />}
				tooltip={{ content: paragraphTextTooltips.alignCenter }}
				classNames={{ root: TOOLBAR_BUTTON_CLASSNAME, active: '' }}
			/>
			<AlignToolbarButton
				value="right"
				icon={<FormatAlignRightIcon />}
				tooltip={{ content: paragraphTextTooltips.alignRight }}
				classNames={{ root: TOOLBAR_BUTTON_CLASSNAME, active: '' }}
			/>
			<AlignToolbarButton
				value="justify"
				icon={<FormatAlignJustifyIcon />}
				tooltip={{ content: paragraphTextTooltips.alignJustify }}
				classNames={{ root: TOOLBAR_BUTTON_CLASSNAME, active: '' }}
			/>
		</>
	);
}

function BasicMarkToolbarButtons() {
	const editor = useCustomPlateEditorRef()!;

	return (
		<>
			<MarkToolbarButton
				type={getPluginType(editor, MARK_BOLD)}
				icon={<FormatBoldIcon />}
				tooltip={{ content: stylesTextTooltips.bold }}
				classNames={{ root: TOOLBAR_BUTTON_CLASSNAME, active: '' }}
			/>
			<MarkToolbarButton
				type={getPluginType(editor, MARK_ITALIC)}
				icon={<FormatItalicIcon />}
				tooltip={{ content: stylesTextTooltips.italic }}
				classNames={{ root: TOOLBAR_BUTTON_CLASSNAME, active: '' }}
			/>
			<MarkToolbarButton
				type={getPluginType(editor, MARK_UNDERLINE)}
				icon={<FormatUnderlinedIcon />}
				tooltip={{ content: stylesTextTooltips.underline }}
				classNames={{ root: TOOLBAR_BUTTON_CLASSNAME, active: '' }}
			/>
			<MarkToolbarButton
				type={getPluginType(editor, MARK_STRIKETHROUGH)}
				icon={<FormatStrikethroughIcon />}
				tooltip={{ content: stylesTextTooltips.strikethrough }}
				classNames={{ root: TOOLBAR_BUTTON_CLASSNAME, active: '' }}
			/>
			<MarkToolbarButton
				type={getPluginType(editor, MARK_SUPERSCRIPT)}
				clear={getPluginType(editor, MARK_SUBSCRIPT)}
				icon={<SuperscriptIcon />}
				tooltip={{ content: stylesTextTooltips.superscript }}
				classNames={{ root: TOOLBAR_BUTTON_CLASSNAME, active: '' }}
			/>
			<MarkToolbarButton
				type={getPluginType(editor, MARK_SUBSCRIPT)}
				clear={getPluginType(editor, MARK_SUPERSCRIPT)}
				icon={<SubscriptIcon />}
				tooltip={{ content: stylesTextTooltips.subscript }}
				classNames={{ root: TOOLBAR_BUTTON_CLASSNAME, active: '' }}
			/>
		</>
	);
}

function InsertsToolbarButtons() {
	const editor = useCustomPlateEditorRef()!;

	return (
		<>
			<BlockToolbarButton
				type={getPluginType(editor, ELEMENT_BLOCKQUOTE)}
				icon={<FormatQuoteIcon />}
				tooltip={{ content: insertsTextTooltips.blockquote }}
				classNames={{ root: TOOLBAR_BUTTON_CLASSNAME, active: '' }}
			/>
			<MarkToolbarButton
				type={getPluginType(editor, MARK_CODE)}
				icon={<CodeAltIcon />}
				tooltip={{ content: insertsTextTooltips.codeBlock }}
				classNames={{ root: TOOLBAR_BUTTON_CLASSNAME, active: '' }}
			/>
		</>
	);
}

function TableToolbarButtons() {
	return (
		<>
			{tableElements.map(({ icon, transform }, index) => (
				<TableToolbarButton
					key={index}
					icon={icon}
					transform={transform}
					classNames={{ root: TOOLBAR_BUTTON_CLASSNAME, active: '' }}
				/>
			))}
		</>
	);
}

export function ToolbarButtons({ canUploadImage = true }: ToolbarButtonsProps) {
	return (
		<ToolbarProvider>
			<GroupToolbarButtonsWrapper
				title={GROUP_TOOLBAR_TITLE_HEADINGS}
				previewIcon={previewIcons[GROUP_TOOLBAR_TITLE_HEADINGS]}
			>
				<BasicElementToolbarButtons />
			</GroupToolbarButtonsWrapper>

			<CustomVerticalRow />

			<GroupToolbarButtonsWrapper
				title={GROUP_TOOLBAR_TITLE_STYLE}
				previewIcon={previewIcons[GROUP_TOOLBAR_TITLE_STYLE]}
			>
				<BasicMarkToolbarButtons />
				<ColorPickerToolbar
					icon={<FormatColorTextIcon />}
					pluginKey={MARK_COLOR}
					tooltip={{ content: FORMAT_COLOR_TEXT_TOOLTIP }}
				/>
				<ColorPickerToolbar
					icon={<HighlightIcon />}
					pluginKey={MARK_BG_COLOR}
					tooltip={{ content: FONT_DOWNLOAD_TEXT_TOOLTIP }}
				/>
			</GroupToolbarButtonsWrapper>

			<CustomVerticalRow />

			<GroupToolbarButtonsWrapper
				title={GROUP_TOOLBAR_TITLE_PARAGRAPH}
				previewIcon={previewIcons[GROUP_TOOLBAR_TITLE_PARAGRAPH]}
			>
				<ListToolbarButtons />
				<IndentToolbarButtons />
				<AlignToolbarButtons />
			</GroupToolbarButtonsWrapper>

			<CustomVerticalRow />

			<GroupToolbarButtonsWrapper
				title={GROUP_TOOLBAR_TITLE_INSERTS}
				previewIcon={previewIcons[GROUP_TOOLBAR_TITLE_INSERTS]}
			>
				<InsertsToolbarButtons />
				<LinkToolbar />
				{canUploadImage && <ImageToolbar />}
			</GroupToolbarButtonsWrapper>

			<CustomVerticalRow />

			<GroupToolbarButtonsWrapper
				title={GROUP_TOOLBAR_TITLE_TABLES}
				previewIcon={previewIcons[GROUP_TOOLBAR_TITLE_TABLES]}
			>
				<TableToolbarButtons />
			</GroupToolbarButtonsWrapper>
		</ToolbarProvider>
	);
}

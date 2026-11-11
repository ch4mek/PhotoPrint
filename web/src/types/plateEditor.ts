import {
	AutoformatRule,
	createPlugins,
	ELEMENT_BLOCKQUOTE,
	ELEMENT_CODE_BLOCK,
	ELEMENT_CODE_LINE,
	ELEMENT_H1,
	ELEMENT_H2,
	ELEMENT_H3,
	ELEMENT_HR,
	ELEMENT_IMAGE,
	ELEMENT_LI,
	ELEMENT_LINK,
	ELEMENT_MEDIA_EMBED,
	ELEMENT_MENTION,
	ELEMENT_MENTION_INPUT,
	ELEMENT_OL,
	ELEMENT_PARAGRAPH,
	ELEMENT_TABLE,
	ELEMENT_TD,
	ELEMENT_TODO_LI,
	ELEMENT_TR,
	ELEMENT_UL,
	OverrideByKey,
	PlateEditor,
	PlatePlugin,
	PluginOptions,
	TElement,
	TImageElement,
	TLinkElement,
	TMediaEmbedElement,
	TMentionElement,
	TMentionInputElement,
	TTableElement,
	TText,
	TTodoListItemElement,
	usePlateEditorRef,
	PlatePluginComponent,
	ELEMENT_H6,
	ELEMENT_H5,
	ELEMENT_H4,
} from '@udecode/plate';
import { CSSProperties } from 'styled-components';

/**
 * Text
 */

type EmptyText = {
	text: '';
};

type PlainText = {
	text: string;
};

interface RichText extends TText {
	bold?: boolean;
	italic?: boolean;
	underline?: boolean;
	strikethrough?: boolean;
	code?: boolean;
	kbd?: boolean;
	subscript?: boolean;
	backgroundColor?: CSSProperties['backgroundColor'];
	fontFamily?: CSSProperties['fontFamily'];
	color?: CSSProperties['color'];
	fontSize?: CSSProperties['fontSize'];
	fontWeight?: CSSProperties['fontWeight'];
}

/**
 * Inline Elements
 */

interface CustomLinkElement extends TLinkElement {
	type: typeof ELEMENT_LINK;
	children: RichText[];
}

interface CustomMentionInputElement extends TMentionInputElement {
	type: typeof ELEMENT_MENTION_INPUT;
	children: [PlainText];
}

interface CustomMentionElement extends TMentionElement {
	type: typeof ELEMENT_MENTION;
	children: [EmptyText];
}

type CustomInlineElement =
	| CustomLinkElement
	| CustomMentionElement
	| CustomMentionInputElement;
type CustomInlineDescendant = CustomInlineElement | RichText;
type CustomInlineChildren = CustomInlineDescendant[];

/**
 * Block props
 */

interface CustomIndentProps {
	indent?: number;
}

interface CustomIndentListProps extends CustomIndentProps {
	listStart?: number;
	listStyleType?: string;
}

interface CustomLineHeightProps {
	lineHeight?: CSSProperties['lineHeight'];
}

interface CustomBlockElement
	extends TElement,
		CustomIndentListProps,
		CustomLineHeightProps {
	id?: string;
}

/**
 * Blocks
 */

interface CustomParagraphElement extends CustomBlockElement {
	type: typeof ELEMENT_PARAGRAPH;
	children: CustomInlineChildren;
}

interface CustomH1Element extends CustomBlockElement {
	type: typeof ELEMENT_H1;
	children: CustomInlineChildren;
}

interface CustomH2Element extends CustomBlockElement {
	type: typeof ELEMENT_H2;
	children: CustomInlineChildren;
}

interface CustomH3Element extends CustomBlockElement {
	type: typeof ELEMENT_H3;
	children: CustomInlineChildren;
}

interface CustomH4Element extends CustomBlockElement {
	type: typeof ELEMENT_H4;
	children: CustomInlineChildren;
}

interface CustomH5Element extends CustomBlockElement {
	type: typeof ELEMENT_H5;
	children: CustomInlineChildren;
}

interface CustomH6Element extends CustomBlockElement {
	type: typeof ELEMENT_H6;
	children: CustomInlineChildren;
}

interface CustomBlockquoteElement extends CustomBlockElement {
	type: typeof ELEMENT_BLOCKQUOTE;
	children: CustomInlineChildren;
}

interface CustomCodeBlockElement extends CustomBlockElement {
	type: typeof ELEMENT_CODE_BLOCK;
	children: CustomCodeLineElement[];
}

interface CustomCodeLineElement extends TElement {
	type: typeof ELEMENT_CODE_LINE;
	children: PlainText[];
}

interface CustomTableElement extends TTableElement, CustomBlockElement {
	type: typeof ELEMENT_TABLE;
	children: CustomTableRowElement[];
}

interface CustomTableRowElement extends TElement {
	type: typeof ELEMENT_TR;
	children: CustomTableCellElement[];
}

interface CustomTableCellElement extends TElement {
	type: typeof ELEMENT_TD;
	children: CustomNestableBlock[];
}

interface CustomBulletedListElement extends TElement, CustomBlockElement {
	type: typeof ELEMENT_UL;
	children: CustomListItemElement[];
}

interface CustomNumberedListElement extends TElement, CustomBlockElement {
	type: typeof ELEMENT_OL;
	children: CustomListItemElement[];
}

interface CustomListItemElement extends TElement, CustomBlockElement {
	type: typeof ELEMENT_LI;
	children: CustomInlineChildren;
}

interface CustomTodoListElement
	extends TTodoListItemElement,
		CustomBlockElement {
	type: typeof ELEMENT_TODO_LI;
	children: CustomInlineChildren;
}

interface CustomImageElement extends TImageElement, CustomBlockElement {
	type: typeof ELEMENT_IMAGE;
	children: [EmptyText];
}

interface CustomMediaEmbedElement
	extends TMediaEmbedElement,
		CustomBlockElement {
	type: typeof ELEMENT_MEDIA_EMBED;
	children: [EmptyText];
}

interface CustomHrElement extends CustomBlockElement {
	type: typeof ELEMENT_HR;
	children: [EmptyText];
}

type CustomNestableBlock = CustomParagraphElement;

export type CustomRootBlock =
	| CustomParagraphElement
	| CustomH1Element
	| CustomH2Element
	| CustomH3Element
	| CustomH4Element
	| CustomH5Element
	| CustomH6Element
	| CustomBlockquoteElement
	| CustomCodeBlockElement
	| CustomTableElement
	| CustomBulletedListElement
	| CustomNumberedListElement
	| CustomTodoListElement
	| CustomImageElement
	| CustomMediaEmbedElement
	| CustomLinkElement
	| CustomHrElement;

export type CustomValue = CustomRootBlock[];

/**
 * Editor types
 */

export type CustomEditor = PlateEditor<CustomValue> & { isDragging?: boolean };

/**
 * Plate types
 */

type CustomOverrideByKey = OverrideByKey<CustomValue, CustomEditor>;

export type CustomPlatePlugin<P = PluginOptions> = PlatePlugin<
	P,
	CustomValue,
	CustomEditor
>;

export interface DiscountNode {
	text?: string;
	children?: DiscountNode[];
	[key: string]: unknown;
}

/**
 * Plate store, Slate context
 */
export const useCustomPlateEditorRef = (id?: string) =>
	usePlateEditorRef<CustomValue, CustomEditor>(id);

/**
 * Utils
 */
export const createCustomPlugins = (
	plugins: CustomPlatePlugin[],
	options?: {
		components?: Record<string, PlatePluginComponent>;
		overrideByKey?: CustomOverrideByKey;
	}
) => createPlugins<CustomValue, CustomEditor>(plugins, options);

export type CustomAutoformatRule = AutoformatRule<CustomValue, CustomEditor>;

export const isTextChild = (child: unknown): child is { text: string } =>
	typeof child === 'object' &&
	child !== null &&
	'text' in child &&
	typeof (child as any).text === 'string';

export enum MediaType {
	Image = 'Image',
	Video = 'Video',
	Link = 'Link',
}

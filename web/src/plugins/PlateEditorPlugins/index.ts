import {
	createPlateUI,
	createAlignPlugin,
	createAutoformatPlugin,
	createBlockquotePlugin,
	createBoldPlugin,
	createCodeBlockPlugin,
	createCodePlugin,
	createExitBreakPlugin,
	createHeadingPlugin,
	createHighlightPlugin,
	createKbdPlugin,
	createImagePlugin,
	createItalicPlugin,
	createLinkPlugin,
	createListPlugin,
	createMediaEmbedPlugin,
	createNodeIdPlugin,
	createParagraphPlugin,
	createResetNodePlugin,
	createSelectOnBackspacePlugin,
	createSoftBreakPlugin,
	createDndPlugin,
	createStrikethroughPlugin,
	createSubscriptPlugin,
	createSuperscriptPlugin,
	createTablePlugin,
	createTodoListPlugin,
	createTrailingBlockPlugin,
	createUnderlinePlugin,
	createComboboxPlugin,
	createMentionPlugin,
	createIndentPlugin,
	createFontColorPlugin,
	createFontBackgroundColorPlugin,
	createDeserializeMdPlugin,
	createDeserializeCsvPlugin,
	createFontSizePlugin,
	createHorizontalRulePlugin,
	createDeserializeDocxPlugin,
	AutoformatPlugin,
	ELEMENT_CODE_BLOCK,
	StyledElement,
} from '@udecode/plate';
import { createJuicePlugin } from '@udecode/plate-juice';

import { normalizeColorStyles } from '@/plugins/PlateEditorPlugins/normalizeColorStyles';
import { tableStyles } from '@/plugins/PlateEditorPlugins/tableStyles';
import { ThemeMode } from '@/theme';
import {
	createCustomPlugins,
	CustomEditor,
	CustomPlatePlugin,
	CustomValue,
} from '@/types/plateEditor';

import { CONFIG } from './config';
import { withStyledDraggables } from './withStyledDraggables';
import { withStyledPlaceHolders } from './withStyledPlaceHolders';

export const getCustomPluginsWithThemeMode = (
	themeMode: ThemeMode,
	isLightTheme?: boolean
) => {
	let components = createPlateUI({
		[ELEMENT_CODE_BLOCK]: StyledElement,
		...tableStyles,
		...normalizeColorStyles(isLightTheme ? ThemeMode.LIGHT : themeMode),
	});
	components = withStyledPlaceHolders(components);
	components = withStyledDraggables(components);

	const customPlugins = createCustomPlugins(
		[
			createParagraphPlugin(),
			createBlockquotePlugin(),
			createTodoListPlugin(),
			createHeadingPlugin(),
			createImagePlugin(),
			createHorizontalRulePlugin(),
			createLinkPlugin(),
			createListPlugin(),
			createTablePlugin(),
			createMediaEmbedPlugin(),
			createCodeBlockPlugin(),
			createAlignPlugin(CONFIG.align),
			createBoldPlugin(),
			createCodePlugin(),
			createItalicPlugin(),
			createHighlightPlugin(),
			createUnderlinePlugin(),
			createStrikethroughPlugin(),
			createSubscriptPlugin(),
			createSuperscriptPlugin(),
			createFontColorPlugin(),
			createFontBackgroundColorPlugin(),
			createFontSizePlugin(),
			createKbdPlugin(),
			createNodeIdPlugin(),
			createDndPlugin(),
			createIndentPlugin(CONFIG.indent),
			createAutoformatPlugin<
				AutoformatPlugin<CustomValue, CustomEditor>,
				CustomValue,
				CustomEditor
			>(CONFIG.autoformat),
			createResetNodePlugin(CONFIG.resetBlockType),
			createSoftBreakPlugin(CONFIG.softBreak),
			createExitBreakPlugin(CONFIG.exitBreak),
			createTrailingBlockPlugin(CONFIG.trailingBlock),
			createSelectOnBackspacePlugin(CONFIG.selectOnBackspace),
			createComboboxPlugin(),
			createMentionPlugin(),
			createDeserializeMdPlugin(),
			createDeserializeCsvPlugin(),
			createDeserializeDocxPlugin(),
			createJuicePlugin() as CustomPlatePlugin,
		],
		{
			components,
		}
	);

	return customPlugins;
};

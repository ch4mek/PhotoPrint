import { parseSync } from 'oxc-parser';
import type { Plugin } from 'vite';

interface StripTestAttributesPluginOptions {
	attributes?: readonly string[];
}

interface AttributeRemovalRange {
	start: number;
	end: number;
	shouldRemoveAdjacentComma: boolean;
}

interface AstNodeLike {
	type?: string;
	start?: number;
	end?: number;
	name?: string | AstNodeLike;
	key?: AstNodeLike;
	value?: unknown;
	computed?: boolean;
	[key: string]: unknown;
}

const PLUGIN_NAME = 'vite-plugin-strip-test-attributes';

const DEFAULT_TEST_ATTRIBUTES = [
	'data-testid',
	'data-test-id',
	'dataTestId',
] as const;

const SOURCE_FILE_RE = /\.[cm]?[jt]sx?$/;
const WHITESPACE_RE = /\s/;

const getFileName = (id: string): string => id.split('?')[0];

const shouldTransformSourceFile = (id: string): boolean => {
	const fileName = getFileName(id);

	return SOURCE_FILE_RE.test(fileName) && !fileName.includes('/node_modules/');
};

const containsTestAttribute = (
	code: string,
	attributes: readonly string[]
): boolean => attributes.some((attribute) => code.includes(attribute));

const getNodeAttributeName = (node: AstNodeLike): string | null => {
	if (node.type === 'JSXAttribute') {
		const name = node.name as AstNodeLike | undefined;

		return name?.type === 'JSXIdentifier' && typeof name.name === 'string'
			? name.name
			: null;
	}

	const key = node.key;

	if (!key) {
		return null;
	}

	if (!node.computed && key.type === 'Identifier') {
		return typeof key.name === 'string' ? key.name : null;
	}

	if (
		(key.type === 'Literal' || key.type === 'StringLiteral') &&
		typeof key.value === 'string'
	) {
		return key.value;
	}

	return null;
};

const traverseAst = (
	node: unknown,
	visitNode: (node: AstNodeLike) => void
): void => {
	if (!node || typeof node !== 'object') {
		return;
	}

	const currentNode = node as AstNodeLike;

	visitNode(currentNode);

	Object.values(currentNode).forEach((value) => {
		if (Array.isArray(value)) {
			value.forEach((item) => traverseAst(item, visitNode));

			return;
		}

		traverseAst(value, visitNode);
	});
};

const expandRangeToAdjacentComma = (
	code: string,
	range: AttributeRemovalRange
): Pick<AttributeRemovalRange, 'start' | 'end'> => {
	let nextIndex = range.end;

	while (WHITESPACE_RE.test(code[nextIndex] ?? '')) {
		nextIndex += 1;
	}

	if (code[nextIndex] === ',') {
		return {
			start: range.start,
			end: nextIndex + 1,
		};
	}

	let previousIndex = range.start - 1;

	while (WHITESPACE_RE.test(code[previousIndex] ?? '')) {
		previousIndex -= 1;
	}

	if (code[previousIndex] === ',') {
		return {
			start: previousIndex,
			end: range.end,
		};
	}

	return range;
};

const collectAttributeRemovalRanges = (
	code: string,
	fileName: string,
	attributeNames: ReadonlySet<string>
): AttributeRemovalRange[] => {
	const { program } = parseSync(fileName, code);
	const ranges: AttributeRemovalRange[] = [];

	traverseAst(program, (node) => {
		if (typeof node.start !== 'number' || typeof node.end !== 'number') {
			return;
		}

		const isJsxAttribute = node.type === 'JSXAttribute';
		const isObjectProperty =
			node.type === 'Property' || node.type === 'ObjectProperty';

		if (!isJsxAttribute && !isObjectProperty) {
			return;
		}

		const attributeName = getNodeAttributeName(node);

		if (!attributeName || !attributeNames.has(attributeName)) {
			return;
		}

		ranges.push({
			start: node.start,
			end: node.end,
			shouldRemoveAdjacentComma: isObjectProperty,
		});
	});

	return ranges;
};

const removeAttributeRanges = (
	code: string,
	ranges: AttributeRemovalRange[]
): string => {
	let result = code;

	ranges
		.map((range) =>
			range.shouldRemoveAdjacentComma
				? expandRangeToAdjacentComma(code, range)
				: range
		)
		.sort((a, b) => b.start - a.start)
		.forEach((range) => {
			result = result.slice(0, range.start) + result.slice(range.end);
		});

	return result;
};

const stripTestAttributesFromCode = (
	code: string,
	fileName: string,
	attributeNames: ReadonlySet<string>
) => {
	const ranges = collectAttributeRemovalRanges(code, fileName, attributeNames);

	if (!ranges.length) {
		return null;
	}

	return {
		code: removeAttributeRanges(code, ranges),
		map: null,
	};
};

export function stripTestAttributesPlugin({
	attributes = DEFAULT_TEST_ATTRIBUTES,
}: StripTestAttributesPluginOptions = {}): Plugin {
	const attributeNames = new Set(attributes);

	return {
		name: PLUGIN_NAME,
		apply: 'build',
		enforce: 'pre',

		transform(code, id) {
			if (
				!shouldTransformSourceFile(id) ||
				!containsTestAttribute(code, attributes)
			) {
				return null;
			}

			try {
				return stripTestAttributesFromCode(
					code,
					getFileName(id),
					attributeNames
				);
			} catch (error) {
				this.warn(
					`${PLUGIN_NAME}: failed to transform ${id}. ${
						error instanceof Error ? error.message : String(error)
					}`
				);

				return null;
			}
		},

		renderChunk(code, chunk) {
			if (!containsTestAttribute(code, attributes)) {
				return null;
			}

			try {
				return stripTestAttributesFromCode(
					code,
					chunk.fileName,
					attributeNames
				);
			} catch (error) {
				this.warn(
					`${PLUGIN_NAME}: failed to transform chunk ${chunk.fileName}. ${
						error instanceof Error ? error.message : String(error)
					}`
				);

				return null;
			}
		},
	};
}

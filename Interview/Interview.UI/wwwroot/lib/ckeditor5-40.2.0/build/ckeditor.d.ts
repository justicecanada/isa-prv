/**
 * @license Copyright (c) 2014-2024, CKSource Holding sp. z o.o. All rights reserved.
 * For licensing, see LICENSE.md or https://ckeditor.com/legal/ckeditor-oss-license
 */
import { InlineEditor } from '@ckeditor/ckeditor5-editor-inline';
import { Bold, Italic } from '@ckeditor/ckeditor5-basic-styles';
import type { EditorConfig } from '@ckeditor/ckeditor5-core';
import { Essentials } from '@ckeditor/ckeditor5-essentials';
import { Link } from '@ckeditor/ckeditor5-link';
import { List } from '@ckeditor/ckeditor5-list';
import { Paragraph } from '@ckeditor/ckeditor5-paragraph';
import { Undo } from '@ckeditor/ckeditor5-undo';
declare class Editor extends InlineEditor {
    static builtinPlugins: (typeof Bold | typeof Essentials | typeof Italic | typeof Link | typeof List | typeof Paragraph | typeof Undo)[];
    static defaultConfig: EditorConfig;
}
export default Editor;
